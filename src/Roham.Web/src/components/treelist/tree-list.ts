import {bindingMode, inject, bindable} from 'aurelia-framework';
import {fireEvent} from './../events';
import {ColumnModel, NodeModel} from './node-model';
import {TreeListNode} from './tree-list-node';

@inject(Element)
export class TreeList {
    private templateElement: any;
    suspendEvents = false;
    suspendUpdate = false;
    subscriptions = [];

    @bindable expandOnFocus: boolean = false;

    @bindable selectOnFocus: boolean = false;

    @bindable columns: ColumnModel[];

    @bindable nodes: NodeModel[];

    @bindable multiSelect: boolean = false;

    @bindable({
        defaultBindingMode: bindingMode.twoWay
    }) focused: NodeModel = null;

    @bindable({
        defaultBindingMode: bindingMode.twoWay
    }) selected: NodeModel[] = [];

    @bindable({
        defaultBindingMode: bindingMode.twoWay
    }) dblclick: NodeModel = null;

    @bindable compareEquality = null;    

    constructor(private element) {
        this.compareEquality = (args) => { return args.a === args.b; };

        let templateElement = this.element.querySelector('tree-node-template');
        if (templateElement) {
            this.templateElement = templateElement;
        } else {
            console.warn('Ctor - no template element');
        }
    }

    created() {
        if (this.templateElement) {
            if (this.templateElement.au) {
                let viewModel = this.templateElement.au.controller.viewModel;                
            } else {
                console.warn('No viewmodel found for template', this.templateElement);
            }
        } else {
            console.warn('Created - no template element');
        }
    }

    attached() {}

    detached() { }

    fireDblClick(node: NodeModel) {
        fireEvent(this.element, 'dblclick', { node });
    }

    focusNode(node: NodeModel, modifiers = {}) {
        if (!this.suspendUpdate) {
            if (node !== this.focused) {
                try {
                    this.suspendUpdate = true;
                    if (this.focused) {
                        this.focused.focused = false;
                    }
                }
                finally {
                    this.suspendUpdate = false;
                }

                this.focused = node;
                fireEvent(this.element, 'focused', { node });

                if (this.expandOnFocus) {
                    node.expandNode();
                }

                if (!this.multiSelect) {
                    try {
                        this.suspendEvents = true;
                        if (this.selected) {
                            this.selected.forEach(node => node.selected = false);
                        }
                    }
                    finally {
                        this.suspendEvents = false;
                    }
                    node.selected = true;
                }
            }

            if (this.selectOnFocus) {
                node.selected = !node.selected;
                if (modifiers['ctrl']) {
                    let recurse = !!modifiers['shift'];
                    node.selectChildren(recurse);
                }
            }
        }
    }

    selectNode(node: NodeModel) {
        let existing = this.selected.findIndex(n => this.compareEquality({ a: node, b: n }));
        if (existing === -1) {
            this.selected.push(node);
            if (!this.suspendEvents) {
                fireEvent(this.element, 'selection-changed', { nodes: this.selected });
            }
        }
    }

    deselectNode(node: NodeModel) {
        let index = this.selected.findIndex(n => this.compareEquality({ a: node, b: n }));
        if (index !== -1) {            
            this.selected.splice(index, 1);
            if (!this.suspendEvents) {
                fireEvent(this.element, 'selection-changed', { nodes: this.selected });
            }
        } 
    }

    clearSelection() {
        this.selected.forEach(node => {
            node.selected = false;
        });
        if (this.focused) {
            this.focused.focused = false;
        }
    }

    findParentTreeNode(node: TreeListNode): TreeListNode {
        let parent: any = node.element.parentNode;
        let parentModel = null;

        return parentModel;
    }

    moveTreeNode(node: TreeListNode, target: TreeListNode | TreeListNode, sibling: TreeListNode) {
        if (target instanceof TreeListNode) {
            target.insertChild(node.model, sibling ? sibling.model : null);
            let parent: any = this.findParentTreeNode(node);
            if (parent === null) {
                parent = this;
                parent.removeNode(node);
            } else {
                parent.removeChild(node.model);
            }
        } else if (target instanceof TreeList) {
            let posNode = this.nodes.indexOf(node.model);
            let posSibling = sibling
                ? this.nodes.indexOf(sibling.model)
                : this.nodes.length - 1;
            if (posNode > -1 && posSibling > -1) {
                this.nodes.splice(posNode, 1);
                this.nodes.splice(posSibling, 0, node.model);
            } else if (posSibling > -1) {
                // move from node to TreeView
                let parent = this.findParentTreeNode(node);
                // parent.removeNode(node);
                parent.removeChild(node.model);
                this.nodes.splice(posSibling, 0, node.model);
            } else {
                console.warn('Sibling not found');
            }
        }
    }

    removeTreeNode(node: TreeListNode) {
        let pos = this.nodes.indexOf(node.model);
        this.nodes.splice(pos, 1);
    }

    nodesChanged(newValue, oldValue) {
        if (newValue) {
            this.enhanceNodes(newValue);
            this.preselectNodes(newValue);

            // focus on first node
            if (newValue.length > 0) {
                newValue[0].focused = true;
            }
        }
    }

    expandOnFocusChanged(newValue) {
        this.expandOnFocus = (newValue === true || newValue === 'true');
    }

    private enhanceNodes = (nodes: NodeModel[]) => {
        nodes.forEach(node => {
            if (node.children && typeof node.children !== 'function') {
                this.enhanceNodes(node.children);
            }
            if (this.templateElement) {
                node.template = this.templateElement.au.controller.viewModel.template;
                node.templateModel = this.templateElement.au.controller.viewModel.model;
            }   
            // TODO:            
            //var tree: any = {
            //    focusNode: this.focusNode.bind(this),
            //    selectNode: this.selectNode.bind(this),
            //    deselectNode: this.deselectNode.bind(this),
            //    multiSelect: this.multiSelect
            //};
            node.tree = this;
        });
    }

    private preselectNodes = (nodes: NodeModel[]) => {
        nodes.forEach(node => {
            if (this.selected.find(n => this.compareEquality({ a: node, b: n }))) {
                node.selected = true;
                node.expandNode().then(() => {
                    this.preselectNodes(node.children);
                });
            }
        });        
    }

    private findClosest(el, tag) {
        tag = tag.toUpperCase();
        do {
            if (el.nodeName.toUpperCase() === tag && el.className.includes('tree-list-row')) {
                // tag name is found! let's return it. :)
                return el;
            }
        } while (el = el.parentNode);
    }
}