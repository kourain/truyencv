using System.ComponentModel;

public enum CategoryType
{
    // [Description("Loại")] MainType = 1,
    [Description("Thể loại")] Genre = 1,
    [Description("Bối cảnh thế giới")] WorldTheme = 2,
    [Description("Nhân vật chính")] MainCharacter = 3,
    [Description("Trường phái")] Class = 4
}