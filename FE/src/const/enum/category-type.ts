export enum CategoryType {
	MainType = 1,
	Genre = 2,
	WorldTheme = 3,
	MainCharacter = 4,
	Class = 5,
}

export const CategoryTypeLabel: Record<CategoryType, string> = {
  [CategoryType.MainType]: "Loại",
  [CategoryType.Genre]: "Thể loại",
  [CategoryType.WorldTheme]: "Bối cảnh thế giới",
  [CategoryType.MainCharacter]: "Nhân vật chính",
  [CategoryType.Class]: "Trường phái"
};
