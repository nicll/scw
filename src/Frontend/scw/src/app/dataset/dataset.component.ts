
import {AfterViewInit, Component, Input, OnInit, ViewChild} from '@angular/core';
import { TableService } from '../Services/table.service';
import { DataSet } from '../Models/DataSet';
import { UserService } from '../Services/user.service';

import {Table} from '../Models/Table';

@Component({
  selector: 'app-dataset',
  templateUrl: './dataset.component.html',
  styleUrls: ['./dataset.component.scss']
})
export class DatasetComponent implements AfterViewInit, OnInit{
  selectedRows: any;

  @Input() tableId:string|undefined;

  constructor(public table:TableService, public user:UserService) { }

  ngAfterViewInit(): void {}
  data= [{A:"dataset11",B:"test21",C:"test31",D:"test41"},{A:"dataset12",B:"test2",C:"test3",D:"test4"},{A:"dataset13",B:"test2",C:"test3",D:"test4"},{A:"dataset14",B:"test2",C:"test3",D:"test4"}];
  cols= [{field:"A", header:"A"},{field:"B", header:"B"},{field:"C", header:"C"},{field:"D", header:"D"}];
  dataset: Array<Array<any>>=new Array;
  rowData: any;


  ngOnInit(): void {
    console.log(this.data[0]["A"]);
  }
  deleteSheet(){
    this.user.DeleteDataSet(this.tableId!).subscribe(data=>console.log("Sheet deleted"))
  }

  saveSheet() {

  }

  onKey($event: MouseEvent) {

  }
}

