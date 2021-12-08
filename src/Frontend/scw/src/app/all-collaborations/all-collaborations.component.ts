import { CollaborationsService } from './../Services/collaborations.service';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { DataSetDialogComponent } from '../Dialogs/data-set-dialog/data-set-dialog.component';
import { Table } from '../Models/Table';
import { UserService } from '../Services/user.service';

@Component({
  selector: 'app-all-collaborations',
  templateUrl: './all-collaborations.component.html',
  styleUrls: ['./all-collaborations.component.scss'],
})
export class AllCollaborationsComponent implements OnInit {
  tabs: Array<Table> = new Array<Table>();
  constructor(private collabs: CollaborationsService, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.collabs.GetCollabs().subscribe(tables=>this.tabs=this.tabs.concat(tables))
  }

  public AddDataset() {
    this.dialog.open(DataSetDialogComponent, { width: '500px' }); //Set width because, column names are too long
  }
}
