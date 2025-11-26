type FontSize = 12 | 14 | 18 | 20 | 24 | 28 | 30;
type FontFamily = "default" | "palatino" | "times" | "arial" | "verdana" | "tahoma" | "comic" | "courier";
type Theme = "light" | "dark" | "sepia";
type LineHeight = "relaxed" | "normal" | "loose";

interface ReadingSettings {
  font_size: FontSize;
  font_family: FontFamily;
  theme: Theme;
  line_height: LineHeight;
}
