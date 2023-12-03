import {ComponentPage, ComponentProp, ComponentWidget, Path, WidgetName} from "./type/type.ts";

const layoutBuilder = Object.freeze({
    widgets: new Map<WidgetName, { default: null|ComponentWidget<ComponentProp>, overwrite: null|ComponentWidget<ComponentProp> }>,
    pages:   new Map<Path,       { default: null|ComponentPage<ComponentProp>,   overwrite: null|ComponentPage<ComponentProp>   }>,

    addDefaultWidget: function<prop extends ComponentProp>(com: ComponentWidget<prop>) {
        if (this.widgets.has(com.name)) {
            throw new Error("Can Not Add New Default Widget If Map Key Exist");
        }

        this.widgets.set(com.name, { default: com as ComponentWidget<ComponentProp>, overwrite: null });

        return this;
    },

    addDefaultPage: function<prop extends ComponentProp>(page: ComponentPage<prop>) {
        if (this.pages.has(page.path)) {
            throw new Error("Can Not Add New Default page If Map Key Exist");
        }

        this.pages.set(page.path, { default: page as ComponentPage<ComponentProp>, overwrite: null });

        return this;
    },

    addWidget: function<prop extends ComponentProp>(com: ComponentWidget<prop>) {
        if (!this.widgets.has(com.name)) {
            this.widgets.set(com.name, { default: null, overwrite: null });
        }

        const obj = this.widgets.get(com.name)!;
        obj.overwrite = com as ComponentWidget<ComponentProp>;
    },

    addPage: function<prop extends ComponentProp>(page: ComponentPage<prop>) {
        if (this.pages.has(page.path)) {
            this.pages.set(page.path, { default: null, overwrite: null });
        }

        const obj = this.pages.get(page.path)!;
        obj.overwrite = page as ComponentPage<ComponentProp>;

        return this;
    },

    build: function () {
        const pages =   new Map<Path,       ComponentPage<ComponentProp>>;
        const widgets = new Map<WidgetName, ComponentWidget<ComponentProp>>;

        this.pages.forEach((v, k) => {
            const value = v.overwrite == null? v.default: v.overwrite;
            if (value == null) {
                throw new Error("Overwrite And Default Is Null");
            }

            pages.set(k, value)
        });

        this.widgets.forEach((v, k) => {
            const value = v.overwrite == null? v.default: v.overwrite;
            if (value == null) {
                throw new Error("Overwrite And Default Is Null");
            }

            widgets.set(k, value);
        });

        return {
            pages: pages,
            widget: widgets
        };
    }
});

export function getInstanceLayoutBuilder() {
    return layoutBuilder;
}