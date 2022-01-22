
import { Injectable } from "@angular/core";
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
  UrlTree
} from "@angular/router";
import { AuthService } from "./auth.service";
import {UserService} from "./user.service";
import {resolve} from "@angular/compiler-cli/src/ngtsc/file_system";
import {Observable} from "rxjs";

@Injectable()
export class LoginGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private user: UserService,
    private router: Router) {
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | UrlTree | boolean  {
    return new Observable<boolean>(obs => {
      this.user.GetUserName().subscribe(
        (data) => {
          console.log("LoginGuard" + "  canActivate")
          console.log("------------------" , data)
          if (data.length > 0) {
            obs.next(true);
          }
        }, error => {
          console.log("LoginGuard" + "  canActivate" + "  error")
          console.log("------------------" + error)
          this.router.navigate(['/'])
          obs.next(false);
        }
      );
    });
  }
}
