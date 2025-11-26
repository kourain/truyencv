const STORAGE_KEY = "reading-settings";

export const DEFAULT_READING_SETTINGS: ReadingSettings = {
  font_size: 18, // Default font size 18px
  font_family: "default",
  theme: "light",
  line_height: "normal",
};

/**
 * Load reading settings from localStorage
 */
export function loadReadingSettings(): ReadingSettings {
  if (typeof window === "undefined") {
    return DEFAULT_READING_SETTINGS;
  }

  try {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (!stored) {
      return DEFAULT_READING_SETTINGS;
    }

    const parsed = JSON.parse(stored) as ReadingSettings;
    // Validate the parsed settings
    return {
      font_size: parsed.font_size || DEFAULT_READING_SETTINGS.font_size,
      font_family: parsed.font_family || DEFAULT_READING_SETTINGS.font_family,
      theme: parsed.theme || DEFAULT_READING_SETTINGS.theme,
      line_height: parsed.line_height || DEFAULT_READING_SETTINGS.line_height,
    };
  } catch (error) {
    console.error("Failed to load reading settings:", error);
    return DEFAULT_READING_SETTINGS;
  }
}

/**
 * Save reading settings to localStorage
 */
export function saveReadingSettings(settings: ReadingSettings): void {
  if (typeof window === "undefined") {
    return;
  }

  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(settings));
  } catch (error) {
    console.error("Failed to save reading settings:", error);
  }
}

/**
 * Convert reading settings to CSS className and style object
 */
export function getReadingSettingsClassName(settings: ReadingSettings): string {
  const fontFamilyClass = {
    default: "",
    palatino: "font-palatino",
    times: "font-times",
    arial: "font-arial",
    verdana: "font-verdana",
    tahoma: "font-tahoma",
    comic: "font-comic",
    courier: "font-courier",
  }[settings.font_family];

  const themeClass = {
    light: "bg-white text-gray-900",
    dark: "bg-gray-900 text-gray-100",
    sepia: "bg-[#f4ecd8] text-[#5c4a3a]",
  }[settings.theme];

  const lineHeightClass = {
    relaxed: "leading-relaxed",
    normal: "leading-7",
    loose: "leading-loose",
  }[settings.line_height];

  return `${fontFamilyClass} ${themeClass} ${lineHeightClass}`.trim();
}

/**
 * Get font family display name
 */
export function getFontFamilyDisplayName(fontFamily: FontFamily): string {
  const displayNames: Record<FontFamily, string> = {
    default: "Mặc định",
    palatino: "Palatino Linotype",
    times: "Times New Roman",
    arial: "Arial",
    verdana: "Verdana",
    tahoma: "Tahoma",
    comic: "Comic Sans MS",
    courier: "Courier New",
  };

  return displayNames[fontFamily];
}
