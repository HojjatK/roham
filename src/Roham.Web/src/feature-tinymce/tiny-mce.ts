import 'tinymce';
import 'ace';
import {inject, bindable, bindingMode, ObserverLocator} from "aurelia-framework";

@inject(ObserverLocator)
export class TinyMce {

	/*
	  bindable properties of tiny-mce custom element.
	  camelCase properties are presented as snake-case attributes on the view.
	 */
    @bindable({ defaultBindingMode: bindingMode.twoWay }) value = "";
    @bindable height = 250;
    @bindable convertUrls = false;	// i.e. convert-urls.bind="true"
    @bindable menuBar = false;
    @bindable toolBar = "undo redo | styleselect | bold forecolor | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link plugin_sample insert_image";
    @bindable contextMenu = "copy paste | link image inserttable | cell row column deletetable";
    @bindable statusBar = false;
    @bindable language = 'ja';
    @bindable insertImageParams = {};

    isChangeRedoUndo: boolean = false;
    editor_id = null;
    editor = null;
    subscriptions = [];

    constructor(observerLocator) {
        var self = this;
        self.editor_id = "tiny-mce-" + "1112432"; // UUID.genV4(); // TODO:
        self.subscriptions = [
            observerLocator
                .getObserver(self, 'value')
                .subscribe(newValue => {
                    if (self.editor) {
                        if (self.isChangeRedoUndo && newValue == self.value) {
                            return;
                        }
                        self.editor.setContent(newValue);
                    }
                }),
            observerLocator
                .getObserver(self, 'insertImageParams')
                .subscribe(newValue => self.editor && (self.editor.insertImageParams = newValue))
        ];
    }

    attached() {
        var self = this;
        var tinymceOptions = {
            selector: 'textarea.tinymce-host',
            height: 500,            
            theme: 'modern',
            plugins: [
                'advlist autolink lists link image charmap print preview hr anchor pagebreak',
                'searchreplace wordcount visualblocks visualchars fullscreen',
                'insertdatetime media nonbreaking save table contextmenu directionality',
                'emoticons template paste textcolor colorpicker textpattern imagetools toc snippet ace'
            ],
            toolbar1: 'undo redo | insert | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image',
            toolbar2: 'print preview media | forecolor backcolor emoticons | snippet ace',
            image_advtab: true,
            templates: [
                { title: 'Test template 1', content: 'Test 1' },
                { title: 'Test template 2', content: 'Test 2' }
            ],
            content_css: [         
                '//fonts.googleapis.com/css?family=Lato:300,300i,400,400i',       
                '//www.tinymce.com/css/codepen.min.css'
            ],            
            setup: editor => {
                editor.on('init', e => {
                    self.editor = editor;            
                });
                editor.on('change redo undo', e => {
                    self.value = editor.getContent();
                    self.isChangeRedoUndo = true;
                });
                editor.insertImageParams = self.insertImageParams;
            }
        };
        var t: any = tinymce;
        t.baseURL = '/jspm_packages/github/tinymce/tinymce-dist@4.5.1';
        tinymce.init(tinymceOptions);

        var w: any = window;
        w.tmce = self;
    }

    detached() {
        if (this.editor != undefined) {
            this.editor.destroy();
        }
    }

    getCssLinks() {
        var result = [];
        if (this.editor != undefined) {
            var h = this.editor.getDoc().getElementsByTagName('head')[0];
            var links = h.getElementsByTagName('link');
            for (var i = 0, max = links.length; i < max; i++) {
                if (links[i].href.match('prism')) {
                    result.push(links[i]);
                }
            }
        }
        return result;
    }
}