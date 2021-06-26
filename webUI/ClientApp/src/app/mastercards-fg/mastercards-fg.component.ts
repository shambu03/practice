import { Component, OnInit, Input } from '@angular/core';
import { MastercardService } from './mastercard.service';

@Component({
  selector: 'app-mastercards-fg',
  templateUrl: './mastercards-fg.component.html',
  styleUrls: ['./mastercards-fg.component.css']
})

export class MastercardsFgComponent implements OnInit {

  constructor(private service: MastercardService) { }

  @Input() dataElement: any;
  MessageTypeList: any[] | undefined;
  ElementList: any[] | undefined;


  selectedMessageType: string | undefined;
  existingdata: string | undefined;
  blob: any | undefined;

  ngOnInit(): void {
    this.LoadMessageTypes();
  }

  LoadMessageTypes() {
    this.service.getMessageType().subscribe((data: any) => {
      this.MessageTypeList = data;
    });
  }

  onMessageTypeChange(event: any) {
    this.selectedMessageType = event;
    console.log(this.selectedMessageType);

    this.service.getElements(this.selectedMessageType).subscribe((data: any) => {
      this.ElementList = data;
    });
  }

  removediv(element: HTMLDivElement) {
    element.remove();;
  }


  addToFile(myForm: { form: { value: { inputs: any; }; }; }, element: HTMLDivElement) {
    //alert(element.innerText);
    //var _data = {
    //  "MessageType": myForm.form.value,
    //  "ExistingData": element.innerText
    //}
    this.service.postElements(myForm.form.value).subscribe((data: any) => {
      //element.append(data);
      this.existingdata = data;
      alert('Success');
    });
  }

  generateFile(myForm: { form: { value: { inputs: any; }; }; }) {
    this.service.generateFile(myForm.form.value).subscribe((data: any) => {

      this.blob = new Blob([data], { type: 'text/plain' });

      var downloadURL = window.URL.createObjectURL(data);
      var link = document.createElement('a');
      link.href = downloadURL;
      link.download = "ipmfile.000";
      link.click();

      alert('File Generated');
    });
  }
}
