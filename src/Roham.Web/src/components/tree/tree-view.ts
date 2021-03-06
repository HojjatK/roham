﻿import {bindingMode, inject, bindable} from 'aurelia-framework';
import {NodeModel} from './node-model';
import {TreeNode} from './tree-node';
import {fireEvent} from './../events';

@inject(Element)
export class TreeView {    
    private templateElement: any;

    @bindable expandOnFocus: boolean = false;
    @bindable selectOnFocus: boolean = false;
    @bindable nodes: NodeModel[];
    @bindable multiSelect: boolean = false;
    @bindable({
        defaultBindingMode: bindingMode.twoWay
    }) focused: NodeModel = null;
    @bindable({
        defaultBindingMode: bindingMode.twoWay
    }) selected: NodeModel[] = [];
    subscriptions = [];

    // comparers
    @bindable compareEquality = null;

    constructor(private element) {
        this.compareEquality = (args) => { return args.a === args.b; };

        let templateElement = this.element.querySelector('tree-node-template');
        if (templateElement) {
            this.templateElement = templateElement;
        } else {
            // this.log.warn('ctor - no template element');
            console.log('Warning: ctor - no template element');
        }
    }

    attached() {}

    detached() {}

    created() {
        if (this.templateElement) {
            if (this.templateElement.au) {
                let viewModel = this.templateElement.au.controller.viewModel;                
                console.log('Info: viewModel', viewModel);
            } else {
                console.log('Warning: no viewmodel found for template', this.templateElement);
            }
        } else {
            console.log('Warn: created - no template element');
        }        
    }

    nodesChanged(newValue, oldValue) {
        if (newValue) {
            // && this.templateElement
            this.enhanceNodes(newValue);
            this.preselectNodes(newValue);
        }
    }

    enhanceNodes(nodes: NodeModel[]) {
        nodes.forEach(node => {
            if (node.children && typeof node.children !== 'function') {
                this.enhanceNodes(node.children);
            }
            if (this.templateElement) {
                node._template = this.templateElement.au.controller.viewModel.template;
                node._templateModel = this.templateElement.au.controller.viewModel.model;
            }
            // node._tree = this;
            node._tree = {
                focusNode: this.focusNode.bind(this),
                selectNode: this.selectNode.bind(this),
                deselectNode: this.deselectNode.bind(this),
                multiSelect: this.multiSelect
            };
        });
    }

    preselectNodes(nodes: NodeModel[]) {
        nodes.forEach(node => {
            if (this.selected.find(n => this.compareEquality({ a: node, b: n }))) {
                node.selected = true;
                node.expandNode().then(() => {
                    this.preselectNodes(node.children);
                });
            }
        });
    }

    _suspendEvents = false;
    _suspendUpdate = false;
    focusNode(node: NodeModel, modifiers = {}) {
        if (!this._suspendUpdate) {
            if (node !== this.focused) {
                if (this.focused) {
                    this._suspendUpdate = true;
                    this.focused.focused = false;
                    this._suspendUpdate = false;
                }
                this.focused = node;
                fireEvent(this.element, 'focused', { node });
                if (this.expandOnFocus) {
                    node.expandNode();
                }
                if (!this.multiSelect) {
                    this._suspendEvents = true;
                    this.selected.forEach(node => node.selected = false);
                    this._suspendEvents = false;
                    // this.selected.splice(0);
                    // this.selectNode(node);
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
            console.log('selecting node', node);
            this.selected.push(node);
            if (!this._suspendEvents) {
                fireEvent(this.element, 'selection-changed', { nodes: this.selected });
            }
        }
    }

    deselectNode(node: NodeModel) {
        let index = this.selected.findIndex(n => this.compareEquality({ a: node, b: n }));
        if (index === -1) {
            console.log('Error: node not found in selected', node);
        } else {
            console.log('deselecting node', node);
            this.selected.splice(index, 1);
            if (!this._suspendEvents) {
                fireEvent(this.element, 'selection-changed', { nodes: this.selected });
            }
        }
    }

    expandOnFocusChanged(newValue) {
        this.expandOnFocus = (newValue === true || newValue === 'true');
    }

    clearSelection() {
        this.selected.forEach(node => {
            node.selected = false;
        });
        if (this.focused) {
            this.focused.focused = false;
        }
    }

    // moveNode(node: TreeNode, target: TreeNode | TreeView) {
    //   console.log('moveNode', node, target);
    //   if (target instanceof TreeNode) {
    //     target.model.children.push(node.model);
    //   }
    //   // target.model.children.push(node.model);
    //   let parent = node.element.parentNode;
    //   let children;
    //   while (parent !== null && parent.tagName !== 'TREE-NODE') {
    //     parent = parent.parentNode;
    //   }
    //   if (parent === null) {
    //     children = this.nodes;
    //   } else {
    //     children = parent.au['tree-node'].viewModel.model.children;
    //   }
    //   let pos = children.indexOf(node.model);
    //   children.splice(pos, 1);
    // }

    findParentNode(node: TreeNode): TreeNode {
        let parent : any = node.element.parentNode;
        let parentModel = null;
        while (parent !== null && parent.tagName.toUpperCase() !== 'TREE-NODE') {
            if (parent.tagName.toUpperCase() === 'TREE-VIEW') {
                parent = null;
            } else {
                parent = parent.parentNode;
            }
        }
        if (parent) {
            parentModel = parent.au['tree-node'].viewModel;
        }
        return parentModel;
    }

    moveNode(node: TreeNode, target: TreeNode | TreeView, sibling: TreeNode) {
        //console.log('moveNode', node, target, sibling);

        // if (sibling) { }
        if (target instanceof TreeNode) {
            target.insertChild(node.model, sibling ? sibling.model : null);
            let parent : any = this.findParentNode(node);
            if (parent === null) {
                parent = this;
                parent.removeNode(node);
            } else {
                parent.removeChild(node.model);
            }
        } else if (target instanceof TreeView) {
            let posNode = this.nodes.indexOf(node.model);
            let posSibling = sibling
                ? this.nodes.indexOf(sibling.model)
                : this.nodes.length - 1;
            if (posNode > -1 && posSibling > -1) {
                this.nodes.splice(posNode, 1);
                this.nodes.splice(posSibling, 0, node.model);
            } else if (posSibling > -1) {
                // move from node to TreeView
                let parent = this.findParentNode(node);
                // parent.removeNode(node);
                parent.removeChild(node.model);
                this.nodes.splice(posSibling, 0, node.model);
            } else {
                console.log('Warn: sibling not found');
            }
        }
    }

    removeNode(node: TreeNode) {
        // console.warn('removeNode not implemented');
        let pos = this.nodes.indexOf(node.model);
        this.nodes.splice(pos, 1);
    }
}