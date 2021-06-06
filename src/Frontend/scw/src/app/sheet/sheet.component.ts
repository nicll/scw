import {AfterViewInit, Component, Input, OnInit} from '@angular/core';
import { TableService } from '../Services/table.service';
import { UserService } from '../Services/user.service';


@Component({
  selector: 'app-sheet',
  templateUrl: './sheet.component.html',
  styleUrls: ['./sheet.component.scss']
})
export class SheetComponent implements AfterViewInit, OnInit{
  selectedRows: any;

  @Input() tableId:string|undefined;

  constructor(public table:TableService, public user:UserService) { }

  ngAfterViewInit(): void {}
  data= [{A:"test11",B:"test21",C:"test31",D:"test41"},{A:"test1",B:"test2",C:"test3",D:"test4"},{A:"test1",B:"test2",C:"test3",D:"test4"},{A:"test1",B:"test2",C:"test3",D:"test4"}];
  cols= ["A","B","C","D"];
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
