import { CollaborationsService } from './../Services/collaborations.service';
import { RemoveCollaborationsDialogComponent } from './../Dialogs/remove-collaborations-dialog/remove-collaborations-dialog.component';
import { AddCollaboratorDialogComponent } from './../Dialogs/add-collaborator-dialog/add-collaborator-dialog.component';
import {
  AfterViewInit,
  Component,
  Input,
  OnInit,
  ViewChild,
} from '@angular/core';
import { TableService } from '../Services/table.service';
import { UserService } from '../Services/user.service';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import autoTable from 'jspdf-autotable';
import xlsx from 'xlsx';
import FileSaver from "file-saver";
import {MenuItem} from "primeng/api";
import { SelectItem, FilterService, FilterMatchMode } from "primeng/api";

import { Table } from '../Models/Table';
import { ApolloService } from '../Services/apollo.service';
import { User } from '../Models/User';
import { MatDialog } from '@angular/material/dialog';
import { AddColumnDialogComponent } from '../Dialogs/add-column-dialog/add-column-dialog.component';
import { DeleteColumnDialogComponent } from '../Dialogs/delete-column-dialog/delete-column-dialog.component';

import {FileUploadModule} from 'primeng/fileupload';
import {HttpClientModule} from '@angular/common/http';
import {map} from "rxjs/operators";
import {Column} from "../Models/Column";

@Component({
  selector: 'app-dataset',
  templateUrl: './dataset.component.html',
  styleUrls: ['./dataset.component.scss']
})
export class DatasetComponent implements AfterViewInit, OnInit{
  selectedRows: any;
  columnsTest: any;
  uploadedFiles: any[] = [];
  items: MenuItem[];
  data: any[];
  cache: any[];
  completedColumns: any[];
  cols = [{field: "A", header: "A"}, {field: "B", header: "B"}, {field: "C", header: "C"}, {field: "D", header: "D"}];
  dataset: Array<Array<any
>>=
  new
  Array;
  rowData: any;
  rowIndex: any;
  exportColumns: any[];
  _selectedColumns: any[];

  @Input() tableId:string|undefined;
  contextMenuItems: any;
  matchModeOptions: any;

  constructor(public table: TableService, public user: UserService, public apollo: ApolloService, public dialog: MatDialog,
              public collab: CollaborationsService) {
    this.completedColumns = [];
    this.cache = [{
      tourid: "1",
      report: "testReport",
      tourname: "testName"
    }]
    this.data = [
      {A: "dataset1", B: "test1", C: "test1", D: "test1"},
      {A: "dataset2", B: "test2", C: "test2", D: "test2"},
      {A: "dataset3", B: "test3", C: "test3", D: "test3"},
      {A: "dataset4", B: "test4", C: "test4", D: "test4"}];

    this.columnsTest = [
      {
        dataKey: "tourname",
        header: "tourname"
      },
      {
        dataKey: "tourid",
        header: "tourid"
      }
    ]
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

    let formattedSelectedColumns = this._selectedColumns.map(s => {
        if (s.hasOwnProperty("field")) {
          s.dataKey = s.field;
        }
        return s;
      }
    )


    this.cache.forEach(element => console.log(element));
    const doc = new jsPDF('p', 'pt')
    console.log(formattedSelectedColumns)
    autoTable(doc, {body: this.data, columns: formattedSelectedColumns});
    doc.save('table.pdf')
  }

  exportExcel() {
    const worksheet = xlsx.utils.json_to_sheet(this.data);
    const workbook = {Sheets: {'data': worksheet}, SheetNames: ['data']};
    const excelBuffer: any = xlsx.write(workbook, {bookType: 'xlsx', type: 'array'});
    this.saveAsExcelFile(excelBuffer, "data");
  }

  saveAsExcelFile(buffer: any, fileName: string): void {
    let EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    let EXCEL_EXTENSION = '.xlsx';
    const data: Blob = new Blob([buffer], {
      type: EXCEL_TYPE
    });
    FileSaver.saveAs(data, fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
    let EXCEL_TYPE =
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    let EXCEL_EXTENSION = '.xlsx';
    const data: Blob = new Blob([buffer], {
      type: EXCEL_TYPE,
    });
    FileSaver.saveAs(
      data,
      fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION
    );
  }
  onAddColumn() {
    const dialogRef = this.dialog.open(AddColumnDialogComponent, {
      data: this.tableId,
    });
  }
  onDeleteColumn() {
    const dialogRef = this.dialog.open(DeleteColumnDialogComponent, {
      data: { id: this.tableId, columns: this.cols.map((tmp) => tmp.header) },
    });
  }
  onAddCollaborations() {
    const dialogRef = this.dialog.open(AddCollaboratorDialogComponent, {
      data: this.tableId,
    });
  }
  onRemoveCollaborations() {
    const dialogRef = this.dialog.open(RemoveCollaborationsDialogComponent, {
      data: this.tableId,
    });
  }

  myUploader(event: any, form: any) {
    console.log("test");
    let data: any, header;
    const target: DataTransfer = <DataTransfer>(event.target);

    for (let file of event.files) {
      console.log("file" + file);

      this.uploadedFiles.push(file);
      const reader: FileReader = new FileReader();
      reader.onload = (e: any) => {
        /* read workbook */
        const bstr: string = e.target.result;
        const wb: xlsx.WorkBook = xlsx.read(bstr, {type: 'binary'});

        /* grab first sheet */
        const wsname: string = wb.SheetNames[0];
        const ws: xlsx.WorkSheet = wb.Sheets[wsname];

        /* save data */
        data = xlsx.utils.sheet_to_json(ws);
        console.log("data" + JSON.stringify(data[0]));

        //call the api
        console.log("worksheetname --- " + wsname);

        this.postDataSet(this.completedColumns, wsname)

        for (let i = 0; i < data.length; i++) {

          let x;

          for (x = 0; x < Object.keys(data[i]).length; x++) {
            this.completedColumns[x] = new Column(Object.keys(data[i])[x], "String", true)
          }

          const map1 = new Map();
          for (const [key, value] of Object.entries(data[i])) {
            map1.set(key, '"' + value + '"');
          }

          // data[0].forEach(element => this.completedColumns[])

          /*    console.log(JSON.stringify(wb.SheetNames))

              console.log(JSON.stringify(this.completedColumns[0]))*/


          this.user.GetDataSets().subscribe(y => {
            let element = (y[y.length - 1].tableRefId)

            // @ts-ignore
            this.apollo.lookUpDataSetId(element).subscribe(z => {
              let mapIter = map1.values();
              let keyiter = map1.keys();
              console.log("ELEMENT -----------" + element)
              console.log("MAP1 -------- " + mapIter.next().value)
              console.log("MAP1 -------- " + mapIter.next().value)
              console.log("MAP1 -------- " + mapIter.next().value)
              console.log(".................................")
              console.log("MAP1 -------- " + keyiter.next().value)
              console.log("MAP1 -------- " + keyiter.next().value)
              console.log("MAP1 -------- " + keyiter.next().value)

              // @ts-ignore
              this.apollo.Insert(z, map1).subscribe()
            });

          })

        }


        //@ts-ignore

        //this.user.GetDataSets().subscribe(y => this.apollo.Insert(y[y.length-1].tableRefId, map1).subscribe())

        // console.log(JSON.stringify(element))
        // console.log(wsname);
      };


      reader.readAsBinaryString(event.files[0]);

      reader.onloadend = (e) => {
        // this.spinnerEnabled = false;
        // console.log(Object.keys(data[0]));
        console.log(Object.keys(data[0]))

        //this.dataSheet.next(data)
        form.clear();
      }
    }
  }

  public postDataSet(table: any[], displayNameTable: any) {
    this.user.PostDataSet(new Table(displayNameTable, table)).subscribe()
  }

  openNew() {
/*    this.user.GetDataSets().subscribe(y => {
      let element = (y[y.length - 1].tableRefId)*/
      // @ts-ignore
      this.apollo.lookUpDataSetId(this.tableId).subscribe(z => {
     /*   let map = new Map([
          ["column1", "123.12"],
          ["column2", '"2000-12-30T12:23:45.123Z"']
        ])*/
          // @ts-ignore
          this.apollo.Insert(z, map).subscribe();
        }
      )

  }
}

