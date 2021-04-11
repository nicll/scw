import {AfterViewInit, Component, Input, OnInit, ViewChild} from '@angular/core';
import Handsontable from "handsontable";
import {HotTableRegisterer} from "@handsontable/angular";
import { TableService } from '../Services/table.service';
import { DataSet } from '../Models/DataSet';
import { UserService } from '../Services/user.service';


@Component({
  selector: 'app-sheet',
  templateUrl: './sheet.component.html',
  styleUrls: ['./sheet.component.scss']
})
export class SheetComponent implements AfterViewInit, OnInit{

  @Input() tableId:string|undefined;
  
  constructor(public table:TableService, public user:UserService) { }

  ngAfterViewInit(): void {
    this.hot=this.hotRegisterer.getInstance(this.id)
    if(this.tableId){
      this.table.GetDataSet(this.tableId).
      subscribe(tables=>{
        console.log(tables);
        if(tables.data.length==0)
          this.hot!.loadData([{"A":1},{"A":10},{"A":12}]);
        else
          this.hot!.loadData(tables.data);
        });
    }
  }
  private hotRegisterer = new HotTableRegisterer();
  id = 'hotInstance';

  hot: Handsontable | undefined;
  hotSettings: Handsontable.GridSettings = {
    colHeaders:true,
    formulas:true,
    contextMenu:true,
    rowHeaders:true,
    comments: true,
    undo: true,
    stretchH: 'all',
    maxCols: 255,
    height:"300px",
    fillHandle: {
      autoInsertRow: true
    },
    data: Handsontable.helper.createEmptySpreadsheetData(30,10),
  };

  dataset: Array<Array<any>>=new Array;
  public saveSheet(){
    if(this.tableId&&this.hot){
      console.log(this.hot.getSourceData());
      this.table.DeleteDataSet(this.tableId);
      this.table.PostDataSet(this.tableId,this.hot.getSourceData()).subscribe(res=>console.log(res));
    }
  }
  public onKey(event: any) { // without type info
    // @ts-ignore
    let exportPlugin1 = this.hot.getPlugin('exportFile');
    exportPlugin1.downloadFile('csv', {
      bom: false,
      columnDelimiter: ',',
      columnHeaders: false,
      exportHiddenColumns: true,
      exportHiddenRows: true,
      fileExtension: 'csv',
      filename: 'Handsontable-csv-file_[YYYY]-[MM]-[DD]',
      mimeType: 'text/csv',
      rowDelimiter: '\r\n',
      rowHeaders: true
    });
  };
  ngOnInit(): void {
  }
  deleteSheet(){
    this.user.DeleteDataSet(this.tableId!).subscribe(data=>console.log("Sheet deleted"))
  }
}
