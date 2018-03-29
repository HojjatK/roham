export interface INavItem {
    key: Number;
    name: string,
    title: string,
    level: number,
    parent: string,
    subItems: INavItem[],
}

export interface INavigation {
    title: string;
    navItems: INavItem[];
}