import { CollaborationsService } from './../../Services/collaborations.service';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UserService } from 'src/app/Services/user.service';

@Component({
  templateUrl: './add-collaborator-dialog.component.html',
  styleUrls: ['./add-collaborator-dialog.component.scss'],
})
export class AddCollaboratorDialogComponent implements OnInit {
  username: string = '';
  message: string = '';

  constructor(
    public dialogRef: MatDialogRef<AddCollaboratorDialogComponent>,
    public collab: CollaborationsService,
    public user: UserService,
    @Inject(MAT_DIALOG_DATA) public tableid: string
  ) {}

  ngOnInit(): void {}
  Ok() {
    console.log(this.tableid);
    if (this.tableid && this.username)
      this.user.GetUserId(this.username).subscribe(next=>{
        if(next)
          this.collab.AddCollaborator(this.tableid, next).subscribe(
            (_) => this.dialogRef.close(),
            (err) => this.message=err.error
          );
      },(err:Error)=>this.message=err.message
      );

  }
  Cancel() {
    this.dialogRef.close();
  }
}
