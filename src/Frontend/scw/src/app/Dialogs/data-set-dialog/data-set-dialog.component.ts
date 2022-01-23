import { DataSource } from '@angular/cdk/table';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { Column } from '../../Models/Column';
import { Table } from '../../Models/Table';
import { UserService } from '../../Services/user.service';

let data:Column[]=[  ]

@Component({
  selector: 'app-data-set-dialog',
  templateUrl: './data-set-dialog.component.html',
  styleUrls: ['./data-set-dialog.component.scss']
})

export class DataSetDialogComponent {

  name:string="";
  nullable:boolean=false;
  type:string="";
  @ViewChild(MatTable, {static: true}) table!: MatTable<Column>;
  displayname:string="";
  displayedColumns: string[] = ['name', 'type', 'nullable'];
  dataSource=new MatTableDataSource(data);
  constructor(private user:UserService,public dialogRef: MatDialogRef<DataSetDialogComponent>) {
  }
  message!:string;
  addRow() {
    console.log(this.type)
    if(this.name&&this.type){
      this.dataSource.data.push(new Column(this.name,this.type,this.nullable));
      this.name="";
      this.type="";
      this.nullable=false;
      this.table.renderRows();
    }
  }
  removeRow() {
    if(this.dataSource.data.length>0){
      this.dataSource.data.splice(this.dataSource.data.length-1)
      this.table.renderRows();
    }
  }
  public Ok(){
    this.user.PostDataSet(new Table(this.displayname,this.dataSource.data, new Date())).subscribe(_=>this.dialogRef.close(),err=>this.message=err)
  }
  public Cancel(){
    this.dialogRef.close();
  }

}

