import {JSX} from "react";

export type Path = `/${string}`;
export type WidgetName = string;
export type PageName = string;

export interface ComponentProp {

}

export interface ComponentWidget<Prop extends ComponentProp> {
    name: WidgetName
    fun: (prop: Prop) => JSX.Element | JSX.Element[]
}

export interface ComponentPage<Prop extends ComponentProp> {
    name: PageName,
    path: Path
    fun: (prop: Prop) => JSX.Element | JSX.Element[]
}