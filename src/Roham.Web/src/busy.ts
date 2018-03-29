export class Busy {
    active: boolean = false;
    counter: number = 0;
    on() {
        var self = this;
        self.counter++;
        setTimeout(() => {
            if (self.counter > 0) {
                self.active = this.counter > 0;
            }
        }, 500);         
    }
    off() { 
        var self = this;       
        self.counter--;   
        setTimeout(() => {
            self.active = this.counter > 0;
        }, 500);              
    }
}