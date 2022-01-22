import { Injectable } from '@angular/core';
import {UserService} from "./user.service";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  username:string|undefined;

  constructor(private user: UserService) {
    this.user.GetUserName().subscribe(username=>{this.username=username
      console.log(this.username)});
  }

  ngOnInit(): void {
      }

  public getAuthStatus(): boolean {
    return this.username != undefined;
  }

  public isUserAdmin(): boolean {
    this.user.GetRolesOfUser().subscribe(role => {
      return role.includes("Admin");
    });
return false;
  }
}
