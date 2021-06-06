import { Component, OnInit } from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import { Router } from '@angular/router';
import { LogInSignUpDialogComponent } from './log-in-sign-up-dialog/log-in-sign-up-dialog.component';
import { UserService } from './Services/user.service';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit{
  title = 'scw';
  username:string|undefined;
  tables:any=["test","test2"];
  constructor(public dialog: MatDialog, private user: UserService,private router: Router){}
  ngOnInit(): void {
    this.user.GetUser().subscribe(user=>{console.log(user);this.username=user.username});
  }
  public Login(){
    const dialogRef = this.dialog.open(LogInSignUpDialogComponent, {
      data: "login"
    });

    dialogRef.afterClosed().subscribe(()=>{
      let usern;
      if(usern=this.user.GetUser()){
        usern.subscribe(us=>this.username=us.username);
      }
    });
  }
  public Signup(){
    const dialogRef = this.dialog.open(LogInSignUpDialogComponent, {
      data: "signup"
    });

    dialogRef.afterClosed().subscribe(()=>{
      let usern;
      if(usern=this.user.GetUser()){
        usern.subscribe(us=>this.username=us.username);
      }
    });
  }
  public ClickAllTables(){
    this.router.navigate(['/tables']);
  }
  public Logout(){
    this.user.Logout();
    this.username=undefined;
  }
}
