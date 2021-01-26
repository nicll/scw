import {AfterViewInit, Component, Input, OnInit, ViewChild} from '@angular/core';
import {HttpService} from '../Services/http.service';
import Handsontable from "handsontable";
import { HotTableComponent } from "@handsontable/angular";
import {HotTableRegisterer} from "@handsontable/angular";


@Component({
  selector: 'app-sheet',
  templateUrl: './sheet.component.html',
  styleUrls: ['./sheet.component.scss']
})
export class SheetComponent implements AfterViewInit,OnInit{

  constructor() { }
  ngAfterViewInit(): void {
    this.hot=this.hotRegisterer.getInstance(this.id)
    console.log("test"+this.hot);
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
    search: true,
    fillHandle: {
      autoInsertRow: true
    },
    data: Handsontable.helper.createEmptySpreadsheetData(20, 20),
  };

  dataset: Array<Array<any>>=new Array;
  tables: string[] = ['T1','T2','T3'];
  searchFiled = document.getElementById('search');

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

}
