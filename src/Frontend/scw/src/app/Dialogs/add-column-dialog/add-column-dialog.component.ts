import { Component, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { Column } from '../../Models/Column';
import { UserService } from '../../Services/user.service';

@Component({
  selector: 'app-add-column-dialog',
  templateUrl: './add-column-dialog.component.html',
  styleUrls: ['./add-column-dialog.component.scss'],
})
export class AddColumnDialogComponent {
  nullable: boolean = false; //can the column be null?
  type: string = ''; //Type of the column: Integer, Real, Timestamp, String
  columnname: string = ''; //Name of the column
  message!: string; //Errormessage

  constructor(
    public dialogRef: MatDialogRef<AddColumnDialogComponent>,
    public user: UserService,
    @Inject(MAT_DIALOG_DATA) public id: string
  ) {}

  public Ok() {
    console.log(this.columnname);
    console.log(this.type);
    console.log(this.nullable);
    this.user
      .PostColumn(
        this.id,
        this.columnname,
        new Column(this.columnname, this.type, this.nullable)
      )
      .subscribe(
        (_) => {
          window.location.reload();
          this.dialogRef.close();
        },
        (err) => (this.message = err)
      );
  }
  public Cancel() {
    this.dialogRef.close();
  }
}
