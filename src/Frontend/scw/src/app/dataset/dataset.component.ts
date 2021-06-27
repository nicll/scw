import {AfterViewInit, Component, Input, OnInit, ViewChild} from '@angular/core';
import { TableService } from '../Services/table.service';
import { UserService } from '../Services/user.service';
import jsPDF from 'jspdf'
import 'jspdf-autotable'
import autoTable from 'jspdf-autotable'
import xlsx from 'xlsx';
import FileSaver from "file-saver";
import {MenuItem} from "primeng/api";
import { SelectItem, FilterService, FilterMatchMode } from "primeng/api";

import {Table} from '../Models/Table';
import { ApolloService } from '../Services/apollo.service';
import { User } from '../Models/User';

@Component({
  selector: 'app-dataset',
  templateUrl: './dataset.component.html',
  styleUrls: ['./dataset.component.scss']
})
export class DatasetComponent implements AfterViewInit, OnInit{
  selectedRows: any;
  items: MenuItem[];
  data: any[];
  cols = [{field:"A", header:"A"},{field:"B", header:"B"},{field:"C", header:"C"},{field:"D", header:"D"}];
  dataset: Array<Array<any>>=new Array;
  rowData: any;
  rowIndex: any;
  exportColumns: any[];
  _selectedColumns: any[];

  @Input() tableId:string|undefined;
  contextMenuItems: any;
  matchModeOptions: any;

  constructor(public table:TableService, public user:UserService, public apollo:ApolloService) {
    this.data= [
      {A:"dataset1",B:"test1",C:"test1",D:"test1"},
      {A:"dataset2",B:"test2",C:"test2",D:"test2"},
      {A:"dataset3",B:"test3",C:"test3",D:"test3"},
      {A:"dataset4",B:"test4",C:"test4",D:"test4"}];
    //this.data=[];
    this.items = [
      {label: 'View', icon: 'pi pi-fw pi-search'},
      {label: 'Delete', icon: 'pi pi-fw pi-times'}
    ];

    this.exportColumns = this.cols.map(col => ({title: col.header, dataKey: col.field}));
    //this._selectedColumns = this.cols;
    this._selectedColumns=[];

  }
  

  @Input() get selectedColumns(): any[] {
    return this._selectedColumns;
  }

  set selectedColumns(val: any[]) {
    //restore original order
    this._selectedColumns = this.cols.filter(col => val.includes(col));
  }

  ngAfterViewInit(): void {
  }

  ngOnInit(): void {
    //load data from graphql
    if(this.tableId != undefined){
      let id=this.tableId;
      this.user.GetDataSet(id).subscribe(dataset=>{//Get details of DataSet
        this.apollo.lookUpDataSetId(id).subscribe(id=>{//Get the GraphqlId
          let query=this.apollo.QueryBuilder(id, dataset.columns.map(v=>v.name))//Build our query
          this.apollo.GetData<any>(query).subscribe(data=>{
            this.data=data.data["all"+this.apollo.makeQueryRightCase(id+"s")].nodes;
            let dataclone: any[] = [];
            this.data.forEach(val => dataclone.push(Object.assign({}, val)));
            dataclone.forEach((element:any) => {
              element["__typename"]=undefined;
            });
            this.data=dataclone;
            console.log(data);
            this.cols=[];
            dataset.columns.forEach((field)=>
              this.cols=this.cols.concat({field:field.name, header:field.name}))
            this._selectedColumns=this.cols; //set the selectedcolumns to all columns in dataset
          });
          //this.apollo.Delete(id,1).subscribe(()=>console.log("delete"));
        });
      });
    }
    const customFilterName = "custom-equals";

    this.matchModeOptions = [
      { label: "Custom Equals", value: customFilterName },
      { label: "Starts With", value: FilterMatchMode.STARTS_WITH },
      { label: "Contains", value: FilterMatchMode.CONTAINS }
    ];
    this.exportColumns = this.cols.map(col => ({title: col.header, dataKey: col.field}));
  }

  deleteSheet(){
    this.user.DeleteDataSet(this.tableId!).subscribe(data=>console.log("Sheet deleted"))
  }

  onEditComplete(event: {field:string, data:any, originalEvent:Event,index:number}): void {
    if(event.index==null||event.index==undefined||!event.field||!event.data||!this.tableId){
      return;
    }
    console.log(event.data);
    this.apollo.lookUpDataSetId(this.tableId).subscribe((id:string)=>{//Get the GraphqlId
      let testdata:Map<string,string>=new Map();
      this.cols.forEach((tmp:{field:string,header:string})=> {
        if(tmp&&tmp.field&&event.data[tmp.field]){
          testdata.set(tmp.field,event.data[tmp.field])
        }
      })

      this.apollo.Update(id,event.index+1,testdata).subscribe(()=>(next:any)=>console.log(next),(err)=>console.log(err));
    });
  }
  saveSheet() {}

  onKey($event: MouseEvent) {}

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

