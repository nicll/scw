import { Component, OnInit } from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import { Router } from '@angular/router';
import { LogInSignUpDialogComponent } from './Dialogs/log-in-sign-up-dialog/log-in-sign-up-dialog.component';
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
    this.user.GetUserName().subscribe(username=>{this.username=username});
  }
  public Login(){
    const dialogRef = this.dialog.open(LogInSignUpDialogComponent, {
      data: "login"
    });

    dialogRef.afterClosed().subscribe(()=>{
      let usern;
      if(usern=this.user.GetUserName()){
        usern.subscribe(username=>this.username=username);
      }
    });
  }
  public Signup(){
    const dialogRef = this.dialog.open(LogInSignUpDialogComponent, {
      data: "signup"
    });

    dialogRef.afterClosed().subscribe(()=>{
      let usern;
      if(usern=this.user.GetUserName()){
        usern.subscribe(username=>this.username=username);
      }
    });
  }
  public ClickAllTables(){
    this.router.navigate(['/tables']);
  }
  public ClickCollabs(){
    this.router.navigate(['/collabs']);
  }
  public Logout(){
    this.user.Logout();
    this.username = undefined;
  }
}
