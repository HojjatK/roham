import {inject, customElement, bindable, noView, processContent, TargetInstruction} from 'aurelia-framework';

@customElement('tree-node-template')
@noView()
@processContent((compiler, resources, element, instruction) => {
    let html = element.innerHTML;
    if (html !== '') {
        instruction.template = html;
    }
    element.innerHTML = '';
})
@inject(TargetInstruction)
export class TreeNodeTemplate {
    @bindable() model;    
    template: any;

    constructor(targetInstruction) {
        this.template = targetInstruction.elementInstruction.template;        
    }
}