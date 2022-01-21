import { Injectable } from '@angular/core';
import {UserService} from "./user.service";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  username:string|undefined;

  constructor(private user: UserService) {

  }

  ngOnInit(): void {
    this.user.GetUserName().subscribe(username=>{this.username=username});
  }

  public getAuthStatus(): boolean {
    return this.username != undefined;
  }
}
