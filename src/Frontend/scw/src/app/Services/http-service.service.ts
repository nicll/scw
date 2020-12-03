import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import sha256, { Hash, HMAC } from "fast-sha256";
@Injectable({
  providedIn: 'root'
})
export class HttpServiceService {
  constructor(private http:HttpClient) { }
  baseUri: string='http://localhost:5000/api';
  public Login(username: string, password: string):Observable<any>{
    //SHA256
    return this.http.post(this.baseUri+"/Service/login",{"username":username,"password":password});
  }
}
