import { Component } from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import { LogInSignUpDialogComponent } from './log-in-sign-up-dialog/log-in-sign-up-dialog.component';
import { UserService } from './Services/user.service';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'scw';
  username:string|undefined;
  tables:any=["test","test2"];
  constructor(public dialog: MatDialog, private user: UserService){}
  public Login(){
    const dialogRef = this.dialog.open(LogInSignUpDialogComponent, {
      data: "login"
    });

    dialogRef.afterClosed().subscribe(()=>{
      let usern;
      if(usern=this.user.GetUser()){
        console.log("te2: "+usern);
        this.username=usern.username;
      }
      
      console.log("te1: "+usern);
    });
  }
  public Signup(){
    const dialogRef = this.dialog.open(LogInSignUpDialogComponent, {
      data: "signup"
    });

    dialogRef.afterClosed().subscribe(()=>{
      let usern;
      if(usern=this.user.GetUser()){
        this.username=usern.username;
      }
    });
  }
}
