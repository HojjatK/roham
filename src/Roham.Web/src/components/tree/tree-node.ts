﻿import {TaskQueue, Container, inject, createOverrideContext, bindable, ViewCompiler, ViewResources, ViewSlot} from 'aurelia-framework';
import {NodeModel} from './node-model';
import {fireEvent} from './../events';

@inject(Element, ViewCompiler, ViewResources, Container, TaskQueue)
export class TreeNode {
    @bindable() model: NodeModel = null;    
    viewSlot: any;
    templateTarget: any;

    constructor(public element: Element, public viewCompiler: ViewCompiler, public viewResources: ViewResources, public container: Container, public taskQueue: TaskQueue) {                
    }

    attached() {
        if (this.model && this.model._template && this.templateTarget) {
            this.useTemplate();
        }
    }

    detached() {
        if (this.viewSlot) {
            this.unbindTemplate();
        }
    }

    insertChild(child: NodeModel, before: NodeModel) {
        let posChild = this.model.children.indexOf(child);
        if (posChild > -1) {
            let posBefore = this.model.children.indexOf(before);
            this.model.children.splice(posChild, 1);
            this.model.children.splice(posBefore, 0, child);
        } else {
            this.model.children.push(child);
        }
        // TODO: insert at position
        // let pos = this.model.children.indexOf(before);
        // if (before) {
        //   let posBefore = this.model.children.indexOf(before);
        //   let posChild = this.model.children.indexOf(child);
        // } else {
        //   this.model.children.push(child);
        // }
    }

    useTemplate() {
        let template = this.model._template;
        let model = this.model._templateModel;
        let viewFactory = this.viewCompiler.compile(`<template>${template}</template>`, this.viewResources);
        let view = viewFactory.create(this.container);
        this.viewSlot = new ViewSlot(this.templateTarget, true);
        this.viewSlot.add(view);
        // this.log.debug('useTemplate, binding model', model);
        // this.viewSlot.bind(this, model);

        // this.viewSlot.bind(this, {
        //   bindingContext: this,
        //   parentOverrideContext: model
        // });

        this.viewSlot.bind(this, createOverrideContext(this, model));
        this.viewSlot.attached();
    }

    unbindTemplate() {
        // @vegarringdal said, switch detached and unbind
        this.viewSlot.detached();
        this.viewSlot.unbind();
        this.viewSlot.removeAll();
    }

    modelChanged(newValue) {
        // this.log.debug('modelChanged', newValue, this.templateTarget);
        if (newValue) {
            newValue._element = this;
            if (newValue._template && this.templateTarget) {
                this.useTemplate();
            }
        }
    }

    // removeNode(node: TreeNode) { }
    removeChild(child: NodeModel) {
        let pos = this.model.children.indexOf(child);
        if (pos > -1) {
            this.model.children.splice(pos, 1);
        } else {
            console.log('Warn: child not found in model', child, this.model.children);
        }
    }

    focusNode(e, permitBubbles) {
        this.model.focused = true;
        return permitBubbles;
    }

    _toggleCalled = false;
    toggleSelected(e, permitBubbles) {
        if (e.ctrlKey) {
            // make sure this is not called twice for checkboxes with child elements (f.i. MDL, Materialize)
            if (!this._toggleCalled) {
                this._toggleCalled = true;
                let promise;
                let newValue = !this.model.selected;
                if (newValue) {
                    promise = this.model.selectChildren(e.shiftKey);
                } else {
                    promise = this.model.deselectChildren(e.shiftKey);
                }
                promise.then(() => {
                    this._toggleCalled = false;
                })
            }
        }
        return permitBubbles || false;
    }

    toggleNode() {
        if (this.model.expanded) {
            this.model.collapseNode();
            fireEvent(this.element, 'collapsed', { node: this.model });
        } else {
            this.model.expandNode();
            fireEvent(this.element, 'expanded', { node: this.model });
        }
    }
}