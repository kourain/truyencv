"use client";

import { X } from "lucide-react";
import {
  DEFAULT_READING_SETTINGS,
  getFontFamilyDisplayName,
  saveReadingSettings,
} from "@helpers/reading-settings";

type ReadingSettingsPanelProps = {
  isOpen: boolean;
  onClose: () => void;
  settings: ReadingSettings;
  onSettingsChange: (settings: ReadingSettings) => void;
};

const ReadingSettingsPanel = ({ isOpen, onClose, settings, onSettingsChange }: ReadingSettingsPanelProps) => {
  if (!isOpen) {
    return null;
  }

  const handleFontSizeChange = (fontSize: FontSize) => {
    const newSettings = { ...settings, font_size: fontSize };
    onSettingsChange(newSettings);
    saveReadingSettings(newSettings);
  };

  const handleFontFamilyChange = (fontFamily: FontFamily) => {
    const newSettings = { ...settings, font_family: fontFamily };
    onSettingsChange(newSettings);
    saveReadingSettings(newSettings);
  };

  const handleThemeChange = (theme: Theme) => {
    const newSettings = { ...settings, theme };
    onSettingsChange(newSettings);
    saveReadingSettings(newSettings);
  };

  const handleLineHeightChange = (lineHeight: LineHeight) => {
    const newSettings = { ...settings, line_height: lineHeight };
    onSettingsChange(newSettings);
    saveReadingSettings(newSettings);
  };

  const handleReset = () => {
    onSettingsChange(DEFAULT_READING_SETTINGS);
    saveReadingSettings(DEFAULT_READING_SETTINGS);
  };

  const fontFamilyOptions: FontFamily[] = [
    "default",
    "palatino",
    "times",
    "arial",
    "verdana",
    "tahoma",
    "comic",
    "courier",
  ];

  return (
    <div
      className="fixed inset-0 z-[1500] flex items-center justify-center bg-black/60 px-4"
      role="dialog"
      aria-modal="true"
      aria-labelledby="settings-panel-title"
    >
      <div className="w-full max-w-lg rounded-2xl border border-surface-muted bg-surface p-6 shadow-2xl">
        <div className="mb-6 flex items-center justify-between">
          <h2 id="settings-panel-title" className="text-xl font-semibold text-primary-foreground">
            Cấu hình đọc truyện
          </h2>
          <button
            type="button"
            onClick={onClose}
            className="rounded-full p-2 text-surface-foreground/60 transition hover:bg-surface-muted/40 hover:text-surface-foreground"
            aria-label="Đóng"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        <div className="space-y-6">
          {/* Font Size */}
          <div>
            <label htmlFor="font-size-select" className="mb-3 block text-sm font-medium text-primary-foreground">
              Kích cỡ chữ
            </label>
            <select
              id="font-size-select"
              value={settings.font_size}
              onChange={(e) => handleFontSizeChange(Number(e.target.value) as FontSize)}
              className="w-full rounded-2xl border border-surface-muted bg-surface px-4 py-2 text-sm text-surface-foreground transition focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20"
            >
              <option value={12}>12px</option>
              <option value={14}>14px</option>
              <option value={18}>18px</option>
              <option value={20}>20px</option>
              <option value={24}>24px</option>
              <option value={28}>28px</option>
              <option value={30}>30px</option>
            </select>
          </div>

          {/* Font Family */}
          <div>
            <label htmlFor="font-family-select" className="mb-3 block text-sm font-medium text-primary-foreground">
              Font chữ
            </label>
            <select
              id="font-family-select"
              value={settings.font_family}
              onChange={(e) => handleFontFamilyChange(e.target.value as FontFamily)}
              className="w-full rounded-2xl border border-surface-muted bg-surface px-4 py-2 text-sm text-surface-foreground transition focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20"
            >
              {fontFamilyOptions.map((font) => (
                <option key={font} value={font}>
                  {getFontFamilyDisplayName(font)}
                </option>
              ))}
            </select>
          </div>

          {/* Theme */}
          <div>
            <label className="mb-3 block text-sm font-medium text-primary-foreground">Màu nền</label>
            <div className="flex flex-wrap gap-2">
              <SettingButton
                label="Sáng"
                isActive={settings.theme === "light"}
                onClick={() => handleThemeChange("light")}
              />
              <SettingButton
                label="Tối"
                isActive={settings.theme === "dark"}
                onClick={() => handleThemeChange("dark")}
              />
              <SettingButton
                label="Sepia"
                isActive={settings.theme === "sepia"}
                onClick={() => handleThemeChange("sepia")}
              />
            </div>
          </div>

          {/* Line Height */}
          <div>
            <label className="mb-3 block text-sm font-medium text-primary-foreground">Khoảng cách dòng</label>
            <div className="flex flex-wrap gap-2">
              <SettingButton
                label="Gần"
                isActive={settings.line_height === "relaxed"}
                onClick={() => handleLineHeightChange("relaxed")}
              />
              <SettingButton
                label="Vừa"
                isActive={settings.line_height === "normal"}
                onClick={() => handleLineHeightChange("normal")}
              />
              <SettingButton
                label="Rộng"
                isActive={settings.line_height === "loose"}
                onClick={() => handleLineHeightChange("loose")}
              />
            </div>
          </div>
        </div>

        {/* Reset Button */}
        <div className="mt-6 flex justify-end gap-3">
          <button
            type="button"
            onClick={handleReset}
            className="rounded-full border border-surface-muted/70 px-5 py-2 text-sm font-medium text-surface-foreground transition hover:bg-surface-muted/40"
          >
            Đặt lại mặc định
          </button>
          <button
            type="button"
            onClick={onClose}
            className="rounded-full bg-primary px-5 py-2 text-sm font-medium text-white transition hover:bg-primary/90"
          >
            Đóng
          </button>
        </div>
      </div>
    </div>
  );
};

type SettingButtonProps = {
  label: string;
  isActive: boolean;
  onClick: () => void;
};

const SettingButton = ({ label, isActive, onClick }: SettingButtonProps) => {
  return (
    <button
      type="button"
      onClick={onClick}
      className={`rounded-full px-4 py-2 text-sm font-medium transition ${isActive
        ? "bg-primary text-white"
        : "border border-primary text-primary hover:bg-primary/10"
        }`}
    >
      {label}
    </button>
  );
};

export default ReadingSettingsPanel;
