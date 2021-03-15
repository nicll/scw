import { DataSource } from '@angular/cdk/table';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { BehaviorSubject, Observable } from 'rxjs';
import { Column } from '../Models/Column';
import { Table } from '../Models/Table';
import { TableService } from '../Services/table.service';
import { UserService } from '../Services/user.service';

@Component({
  selector: 'app-data-set-dialog',
  templateUrl: './data-set-dialog.component.html',
  styleUrls: ['./data-set-dialog.component.scss']
})
export class DataSetDialogComponent implements OnInit {
  
  name:string="";
  nullable:string="false";
  type:string="";

  displayedColumns: string[] = ['name', 'type', 'nullable'];
  dataSource= new ExampleDataSource();
  constructor(private user:UserService,public dialogRef: MatDialogRef<DataSetDialogComponent>) { }
  displayname!:string;

  message!:string;
  addColumn() {
    if(this.name&&this.type&&this.nullable)
      this.dataSource.data.value.push(new Column(this.name,this.type,false));
  }

  removeColumn() {
    if (this.dataSource.data.value.length) {
    }
  }
  ngOnInit(): void {
  } 
  public Ok(){
    if(!this.name||!this.type||!this.nullable||this.dataSource.data.value.length==0)
      return;
    this.user.PostDataSet(new Table(this.displayname,this.dataSource.data.value)).subscribe(_=>this.dialogRef.close(),err=>this.message=err) 
  }
  public Cancel(){
    this.dialogRef.close();
  }

}
export class ExampleDataSource extends DataSource<Column> {
  /** Stream of data that is provided to the table. */
  data = new BehaviorSubject<Column[]>([new Column("A","Integer",false)]);

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<Column[]> {
    return this.data;
  }

  disconnect() {}
}
