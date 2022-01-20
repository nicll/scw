import { CollaborationsService } from './../../Services/collaborations.service';
import { Component, Inject, OnInit } from '@angular/core';
import { User } from 'src/app/Models/User';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  templateUrl: './remove-collaborations-dialog.component.html',
  styleUrls: ['./remove-collaborations-dialog.component.scss']
})
export class RemoveCollaborationsDialogComponent implements OnInit {

  constructor(private dialogRef: MatDialogRef<RemoveCollaborationsDialogComponent>, private collab:CollaborationsService, @Inject(MAT_DIALOG_DATA) public tableid: string) { }
  collaborators:User[]=[];
  errormessage:string="";
  selectedCollaborator?:User=undefined;

  ngOnInit(): void {
    this.collab.GetCollabsForTable(this.tableid).subscribe(
      (next)=>this.collaborators=next,
      (error)=>this.errormessage=error.error
    );
  }
  Ok() {
    console.log(this.collaborators);
    if (this.tableid && this.selectedCollaborator)
    {
      	console.log(this.selectedCollaborator)
        this.collab.RemoveCollabs(this.tableid, this.selectedCollaborator.userId!).subscribe(
          (next) => this.dialogRef.close(),
          (err) => this.errormessage=err.error
        );
    }
  }
  Cancel() {
    this.dialogRef.close();
  }

}
