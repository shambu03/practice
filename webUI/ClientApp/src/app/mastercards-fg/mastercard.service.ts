import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class MastercardService {
  APIUrl = environment.baseUrl;

  constructor(private http: HttpClient) { }

  getMessageType(): Observable<any[]> {
    return this.http.get<any>(this.APIUrl + '/MTI');
  }

  getElements(messageType: any): Observable<any[]> {
    console.log('getElements');
    return this.http.get<any>(this.APIUrl + '/MTI/GetElements?MessageType='+ messageType);
    
  }

  postElements(messageType: any): Observable<any[]> {
    console.log('postElements');
    
    return this.http.post<any>(this.APIUrl + '/MTI/FormatElement' , messageType);

    //var _data = {
    //  "MessageType": messageType,
      
    //}
    //return this.http.post<any>(this.APIUrl + '/MTI/FormatElement', _data);

  }

  generateFile(messageType: any): Observable<any[]> {
    const httpOptions = {
      responseType: 'blob' as 'json'
    };

    //,
    //headers: new HttpHeaders({
    //  'Authorization': this.authKey,
    //})

    return this.http.post<any>(this.APIUrl + '/MTI/CreateFile', messageType, httpOptions);


  }
}
