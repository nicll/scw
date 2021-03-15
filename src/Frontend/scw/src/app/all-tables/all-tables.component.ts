import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { _MatTabLinkBase } from '@angular/material/tabs';
import { DataSetDialogComponent } from '../data-set-dialog/data-set-dialog.component';
import { Table } from '../Models/Table';
import { UserService } from '../Services/user.service';
@Component({
  selector: 'app-all-tables',
  templateUrl: './all-tables.component.html',
  styleUrls: ['./all-tables.component.scss']
})
export class AllTablesComponent implements OnInit {
  tabs:Array<Table>=new Array<Table>();
  selected=new FormControl(0);
  constructor(private user:UserService,private dialog:MatDialog) { }

  ngOnInit(): void {
    this.user.GetSheets().subscribe(tables=>this.tabs=this.tabs.concat(tables));
    this.user.GetDataSets().subscribe(tables=>{this.tabs=this.tabs.concat(tables);console.log(this.tabs);});
  }
  public AddDataset(){
    this.dialog.open(DataSetDialogComponent);
  }

}
