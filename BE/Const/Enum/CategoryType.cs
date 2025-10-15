using System.ComponentModel;

public enum CategoryType
{
    [Description("Loại")] MainType = 1,
    [Description("Thể loại")] Genre = 2,
    [Description("Bối cảnh thế giới")] WorldTheme = 3,
    [Description("Nhân vật chính")] MainCharacter = 4,
    [Description("Trường phái")] Class = 5
}