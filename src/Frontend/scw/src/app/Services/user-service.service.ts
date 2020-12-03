import { Injectable, OnInit } from '@angular/core';
import { User } from '../Models/User';

@Injectable({
  providedIn: 'root'
})
export class UserServiceService {

  constructor() { }
  user?:User=undefined;
  public GetUser():User{
    
  }
}
