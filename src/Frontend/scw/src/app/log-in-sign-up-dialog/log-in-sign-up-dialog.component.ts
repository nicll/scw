import { Component, OnInit, Inject } from '@angular/core';
import { UserService } from '../Services/user.service';
import { MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
@Component({
  selector: 'app-log-in-sign-up-dialog',
  templateUrl: './log-in-sign-up-dialog.component.html',
  styleUrls: ['./log-in-sign-up-dialog.component.scss']
})
export class LogInSignUpDialogComponent {
  constructor(private user:UserService, 
              public dialogRef: MatDialogRef<LogInSignUpDialogComponent>,
              @Inject(MAT_DIALOG_DATA) public data: string) 
  { 
      this.username="";
      this.password="";
  }
  username:string;
  password:string;
  message:string="";
  public Ok(){
    console.log(this.username+" "+this.password)
    if(!this.username||!this.password)
      return;
    this.message="";
    switch(this.data){
      case "login":
        this.user.Login(this.username,this.password).subscribe(
          res=>{console.log(res); this.dialogRef.close();},
          err=>{console.log(err); this.message=err.error});
        break;
      case "signup":
        this.user.Signup(this.username,this.password).subscribe(
          res=>{console.log(res); this.dialogRef.close();},
          err=>{console.log(err); this.message=err.error});
        break;
    }
  }
  public Cancel(){
    this.dialogRef.close();
  }
}
