
import {AfterViewInit, Component, Input, OnInit, ViewChild} from '@angular/core';
import { TableService } from '../Services/table.service';
import { DataSet } from '../Models/DataSet';
import { UserService } from '../Services/user.service';
import jsPDF from 'jspdf'
import 'jspdf-autotable'
import autoTable from 'jspdf-autotable'
import xlsx from 'xlsx';


import {Table} from '../Models/Table';
import FileSaver from "file-saver";

@Component({
  selector: 'app-dataset',
  templateUrl: './dataset.component.html',
  styleUrls: ['./dataset.component.scss']
})
export class DatasetComponent implements AfterViewInit, OnInit{
  selectedRows: any;

  @Input() tableId:string|undefined;
  contextMenuItems: any;

  constructor(public table:TableService, public user:UserService) {
    this.exportColumns = this.cols.map(col => ({title: col.header, dataKey: col.field}));

  }

  ngAfterViewInit(): void {}
  data= [
    {A:"dataset11",B:"test21",C:"test31",D:"test41"},
    {A:"dataset12",B:"test2",C:"test3",D:"test4"},{A:"dataset13",B:"test2",C:"test3",D:"test4"},{A:"dataset14",B:"test2",C:"test3",D:"test4"}];
  cols= [{field:"A", header:"A"},{field:"B", header:"B"},{field:"C", header:"C"},{field:"D", header:"D"}];
  dataset: Array<Array<any>>=new Array;
  rowData: any;
  rowIndex: any;
  exportColumns: any[];


  ngOnInit(): void {
    console.log(this.data[0]["A"]);
    this.exportColumns = this.cols.map(col => ({title: col.header, dataKey: col.field}));

    /*    this.contextMenuItems = [
          {label: 'View', icon: 'pi pi-fw pi-search', command: () => this.debugFunction()},
          {label: 'Delete', icon: 'pi pi-fw pi-times', command: () => this.debugFunction()}
        ];*/
  }
  deleteSheet(){
    this.user.DeleteDataSet(this.tableId!).subscribe(data=>console.log("Sheet deleted"))
  }


  debugFunction():void{
    console.log("debug ContextMenu")
  }

  saveSheet() {

  }

  onKey($event: MouseEvent) {

  }

  exportPdf() {
    const doc = new jsPDF()
    autoTable(doc,{body: this.data, columns: this.exportColumns});
    doc.save('table.pdf')
  }

  exportExcel() {
    const worksheet = xlsx.utils.json_to_sheet(this.data);
    const workbook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
    const excelBuffer: any = xlsx.write(workbook, { bookType: 'xlsx', type: 'array' });
    this.saveAsExcelFile(excelBuffer, "data");
  }

  saveAsExcelFile(buffer: any, fileName: string): void {
    let EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
      let EXCEL_EXTENSION = '.xlsx';
      const data: Blob = new Blob([buffer], {
        type: EXCEL_TYPE
      });
      FileSaver.saveAs(data, fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
  }
}

