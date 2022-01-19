import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UserService } from '../../Services/user.service';

@Component({
  selector: 'app-delete-column-dialog',
  templateUrl: './delete-column-dialog.component.html',
  styleUrls: ['./delete-column-dialog.component.scss'],
})
export class DeleteColumnDialogComponent {
  name: string = '';
  message: string = '';

  constructor(
    public dialogRef: MatDialogRef<DeleteColumnDialogComponent>,
    public user: UserService,
    @Inject(MAT_DIALOG_DATA) public data: { id: string; columns: Array<string> }
  ) {}

  public Ok() {
    console.log(this.data.columns);
    this.user.DeleteColumn(this.data.id, this.name).subscribe(
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
