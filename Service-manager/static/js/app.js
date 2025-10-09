let serviceStates = {}; // Track service states: 'stopped', 'running', 'starting', 'restarting'
let websocket = null;

function connectWebSocket() {
	const protocol = window.location.protocol === 'https:' ? 'wss:' : 'ws:';
	const wsUrl = `${protocol}//${window.location.host}/ws`;

	console.log('Attempting to connect to WebSocket:', wsUrl);

	websocket = new WebSocket(wsUrl);

	websocket.onopen = function (event) {
		console.log('WebSocket connection established');
	};

	websocket.onmessage = function (event) {
		try {
			const data = JSON.parse(event.data);
			console.log('WebSocket data received:', data);
			if (data.type === 'data_update') {
				displayServices(data.services, data.totals);
				if (data.system_info) {
					displaySystemInfo(data.system_info);
				}
			}
		} catch (error) {
			console.error('Error parsing WebSocket message:', error);
		}
	};

	websocket.onclose = function (event) {
		console.log('WebSocket connection closed, attempting to reconnect...');
		setTimeout(connectWebSocket, 2000); // Reconnect after 2 seconds
	};

	websocket.onerror = function (error) {
		console.error('WebSocket error:', error);
	};
}

function displaySystemInfo(systemInfo) {
	const statsContainer = document.getElementById('system-stats');
	statsContainer.innerHTML = `
        <div class="stat-item">
            <div class="stat-label">CPU Usage</div>
            <div class="stat-value">${systemInfo.cpu_percent}%
				<div>->${systemInfo.cpu_count} cores</div>
				<div>->${(systemInfo.cpu_freq / 1000).toFixed(2)} GHz</div>
				<div>->${systemInfo.timestamp}</div>
			</div>
        </div>
        <div class="stat-item">
            <div class="stat-label">Memory Usage</div>
            <div class="stat-value">${systemInfo.memory.percent}%
			 	<div>->${(systemInfo.memory.used / 1024 / 1024 / 1024).toFixed(1)} / ${(systemInfo.memory.total / 1024 / 1024 / 1024).toFixed(1)} GB</div>
				<div> Free ${(systemInfo.memory.available / 1024 / 1024 / 1024).toFixed(1)} GB</div>
			</div>
        </div>
        <div class="stat-item">
            <div class="stat-label">Disk Usage in "${systemInfo.disk.path}"</div>
            <div class="stat-value">${systemInfo.disk.percent}%
			 	<div>->${(systemInfo.disk.used / 1024 / 1024 / 1024).toFixed(1)} / ${(systemInfo.disk.total / 1024 / 1024 / 1024).toFixed(1)} GB</div>
				<div> Free ${(systemInfo.disk.free / 1024 / 1024 / 1024).toFixed(1)} GB</div>
        </div>
    `;
}

function displayServices(servicesArray, totals = null) {
	const container = document.getElementById('services-container');
	if (!servicesArray || servicesArray.length === 0) {
		container.innerHTML = '<div>Không có dịch vụ nào được cấu hình</div>';
		return;
	}

	// Check if table already exists
	let table = container.querySelector('.services-table');
	if (!table) {
		// Create table structure
		container.innerHTML = `
            <table class="services-table">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Working Directory</th>
                        <th>Status</th>
						<th>PID</th>
                        <th>%CPU</th>
                        <th>RAM (MB)</th>
                        <th>%RAM</th>
                        <th>Control</th>
                    </tr>
                </thead>
                <tbody id="services-tbody">
                </tbody>
            </table>
        `;
	}

	const tbody = document.getElementById('services-tbody');
	const existingRows = tbody.querySelectorAll('tr:not(#total-row)');

	// Update or create rows for each service
	servicesArray.forEach(service => {
		const serviceId = service.id;
		let row = document.getElementById(`row-${serviceId}`);

		if (!row) {
			// Create new row
			row = document.createElement('tr');
			row.id = `row-${serviceId}`;
			row.innerHTML = `
                <td class="service-name-cell">${service.name}</td>
                <td class="working-dir-cell" title="${service.workingDirectory || 'N/A'}">${service.workingDirectory || 'N/A'}</td>
                <td class="status-cell">
                    <span class="status-badge status-unknown" id="status-${serviceId}">
                        '●'
                    </span>
                </td>
                <td class="service-name-cell" id="pid-${serviceId}">${service.process_info?.pid || ""}</td>
                <td class="cpu-cell">
                    <span class="cpu-value" id="cpu-${serviceId}">-</span>
                </td>
                <td class="ram-cell">
                    <span class="ram-value" id="ram-${serviceId}">-</span>
                </td>
                <td class="ram-percent-cell">
                    <span class="ram-percent-value" id="ram-percent-${serviceId}">-</span>
                </td>
                <td class="actions-cell">
                    <button class="btn btn-start btn-small" onclick="startService('${serviceId}')" title="Khởi chạy dịch vụ">
                        ▶️
                    </button>
                    <button class="btn btn-stop btn-small" onclick="stopService('${serviceId}')" title="Dừng dịch vụ">
                        ⏹️
                    </button>
                    <button class="btn btn-restart btn-small" onclick="restartService('${serviceId}')" title="Khởi động lại">
                        🔄
                    </button>
                </td>
            `;
			tbody.appendChild(row);
		}

		// Update service data
		updateServiceDisplay(service);
	});

	// Update total row
	if (totals) {
		updateTotalRowData(totals, servicesArray);
	}
}

function updateServiceDisplay(service) {
	const serviceId = service.id;

	const statusElement = document.getElementById(`status-${serviceId}`);
	const pidElement = document.getElementById(`pid-${serviceId}`);
	const cpuElement = document.getElementById(`cpu-${serviceId}`);
	const ramElement = document.getElementById(`ram-${serviceId}`);
	const ramPercentElement = document.getElementById(`ram-percent-${serviceId}`);

	if (statusElement && cpuElement && ramElement && ramPercentElement && pidElement) {
		if (service.process_info) {
			const pidValue = service.process_info.pid;
			const cpuValue = service.process_info.cpu_percent;
			const ramValue = service.process_info.ram_mb;
			const ramPercentValue = service.process_info.ram_percent;

			statusElement.textContent = '●';
			statusElement.className = 'status-badge status-running';
			pidElement.textContent = `${pidValue}`;
			cpuElement.textContent = `${cpuValue.toFixed(1)}%`;
			ramElement.textContent = `${ramValue.toFixed(1)} MB`;
			ramPercentElement.textContent = `${ramPercentValue.toFixed(1)}%`;
			updateButtonStates(serviceId, 'running');
		} else {
			statusElement.textContent = '●';
			statusElement.className = 'status-badge status-stopped';
			pidElement.textContent = "";
			cpuElement.textContent = '-';
			ramElement.textContent = '-';
			ramPercentElement.textContent = '-';
			updateButtonStates(serviceId, 'stopped');
		}
	}
}

function updateButtonStates(serviceId, state) {
	serviceStates[serviceId] = state;

	const startBtn = document.querySelector(`button[onclick="startService('${serviceId}')"]`);
	const stopBtn = document.querySelector(`button[onclick="stopService('${serviceId}')"]`);
	const restartBtn = document.querySelector(`button[onclick="restartService('${serviceId}')"]`);

	if (startBtn && stopBtn && restartBtn) {
		switch (state) {
			case 'stopped':
				startBtn.disabled = false;
				stopBtn.disabled = true;
				restartBtn.disabled = true;
				startBtn.style.opacity = '1';
				stopBtn.style.opacity = '0.5';
				restartBtn.style.opacity = '0.5';
				break;

			case 'running':
				startBtn.disabled = true;
				stopBtn.disabled = false;
				restartBtn.disabled = false;
				startBtn.style.opacity = '0.5';
				stopBtn.style.opacity = '1';
				restartBtn.style.opacity = '1';
				break;

			case 'starting':
			case 'restarting':
				startBtn.disabled = true;
				stopBtn.disabled = true;
				restartBtn.disabled = true;
				startBtn.style.opacity = '0.5';
				stopBtn.style.opacity = '0.5';
				restartBtn.style.opacity = '0.5';
				break;

			default:
				// Unknown state - enable all buttons
				startBtn.disabled = false;
				stopBtn.disabled = false;
				restartBtn.disabled = false;
				startBtn.style.opacity = '1';
				stopBtn.style.opacity = '1';
				restartBtn.style.opacity = '1';
		}
	}
}

function updateTotalRowData(totals, servicesArray) {
	// Remove existing total row if it exists
	const existingTotalRow = document.getElementById('total-row');
	if (existingTotalRow) {
		existingTotalRow.remove();
	}

	const tbody = document.getElementById('services-tbody');
	const runningCount = servicesArray.filter(service => service.process_info).length;

	// Add new total row
	const totalRow = document.createElement('tr');
	totalRow.id = 'total-row';

	totalRow.innerHTML = `
        <td style="color: #ffffff; font-weight: bold;"></td>
        <td style="color: #b0b0b0; font-style: italic; text-align: left;"></td>
        <td style="text-align: left;">
            <span style="color: #5cb85c; font-weight: bold;">${runningCount} ●</span>
        </td>
        <td style="color: #ffffff; font-weight: bold;"></td>
        <td style="text-align: left;">
            <span style="color: #5bc0de; font-weight: bold;">${totals.cpu_percent >= 0 ? totals.cpu_percent.toFixed(1) + '%' : '-'}</span>
        </td>
        <td style="text-align: left;">
            <span style="color: #5cb85c; font-weight: bold;">${totals.ram_mb >= 0 ? totals.ram_mb.toFixed(1) + ' MB' : '-'}</span>
        </td>
        <td style="text-align: left;">
            <span style="color: #f0ad4e; font-weight: bold;">${totals.ram_percent >= 0 ? totals.ram_percent.toFixed(1) + '%' : '-'}</span>
        </td>
        <td style="text-align: left; color: #b0b0b0; font-style: italic;">-</td>
    `;

	tbody.appendChild(totalRow);
}

async function startService(serviceId) {
	try {
		// Set starting state
		updateButtonStates(serviceId, 'starting');
		const statusElement = document.getElementById(`status-${serviceId}`);
		statusElement.textContent = '⏳';
		statusElement.className = 'status-badge status-unknown';

		const response = await fetch(`/services/${serviceId}/start`, { method: 'POST' });
		const result = await response.json();
		if (response.ok) {
			showNotification(`✅ Dịch vụ ${serviceId} đã được khởi chạy!`, 'success');
		} else {
			showNotification(`❌ Lỗi: ${result.detail}`, 'error');
		}

		// WebSocket will handle status updates automatically
	} catch (error) {
		showNotification(`❌ Lỗi khi khởi chạy dịch vụ: ${error.message}`, 'error');
		// Reset to stopped state on error
		updateButtonStates(serviceId, 'stopped');
	}
}

async function stopService(serviceId) {
	try {
		// Set stopping state (similar to starting, disable all buttons)
		updateButtonStates(serviceId, 'starting'); // Reuse starting state for stopping
		const statusElement = document.getElementById(`status-${serviceId}`);
		statusElement.textContent = '⏳';
		statusElement.className = 'status-badge status-unknown';

		const response = await fetch(`/services/${serviceId}/stop`, { method: 'POST' });
		const result = await response.json();
		if (response.ok) {
			showNotification(`✅ Dịch vụ ${serviceId} đã được dừng!`, 'success');
		} else {
			showNotification(`❌ Lỗi: ${result.detail}`, 'error');
		}

		// WebSocket will handle status updates automatically
	} catch (error) {
		showNotification(`❌ Lỗi khi dừng dịch vụ: ${error.message}`, 'error');
		// WebSocket will reset proper button state
	}
}

async function restartService(serviceId) {
	try {
		// Set restarting state
		updateButtonStates(serviceId, 'restarting');
		const statusElement = document.getElementById(`status-${serviceId}`);
		statusElement.textContent = '🔄';
		statusElement.className = 'status-badge status-unknown';

		const response = await fetch(`/services/${serviceId}/restart`, { method: 'POST' });
		const result = await response.json();
		if (response.ok) {
			showNotification(`✅ Dịch vụ ${serviceId} đã được khởi động lại!`, 'success');
		} else {
			showNotification(`❌ Lỗi: ${result.detail}`, 'error');
		}

		// WebSocket will handle status updates automatically
	} catch (error) {
		showNotification(`❌ Lỗi khi khởi động lại dịch vụ: ${error.message}`, 'error');
		// WebSocket will reset proper button state
	}
}

function showNotification(message, type) {
	// Create notification element
	const notification = document.createElement('div');
	notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 15px 20px;
        border-radius: 4px;
        color: white;
        font-weight: bold;
        z-index: 1000;
        max-width: 300px;
        word-wrap: break-word;
        opacity: 0;
        transition: opacity 0.3s ease-in-out;
        ${type === 'success' ? 'background-color: #28a745;' : 'background-color: #dc3545;'}
    `;
	notification.textContent = message;

	document.body.appendChild(notification);

	// Fade in
	setTimeout(() => {
		notification.style.opacity = '1';
	}, 100);

	// Remove after 3 seconds
	setTimeout(() => {
		notification.style.opacity = '0';
		setTimeout(() => {
			document.body.removeChild(notification);
		}, 300);
	}, 3000);
}

// Initialize app when DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
	// Connect to WebSocket for real-time updates
	connectWebSocket();
});
