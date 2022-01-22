import { Injectable } from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from '@angular/router';
import { Observable } from 'rxjs';
import {AuthService} from "./auth.service";
import {UserService} from "./user.service";

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(
    private user: UserService,
    private authService: AuthService,
    private router: Router) { }
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | UrlTree | boolean  {
    return new Observable<boolean>(obs => {
      this.user.GetRolesOfUser().subscribe(role => {
        if(role.includes("Admin")){
          obs.next(true);
        } else {
          this.router.navigate(['/']);
          obs.next(false);
        }
      });
  })
}
}
