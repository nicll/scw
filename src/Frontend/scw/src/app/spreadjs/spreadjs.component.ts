import { Component, ViewEncapsulation } from '@angular/core';
import '@grapecity/spread-sheets-designer-resources-en';
import * as GC from '@grapecity/spread-sheets';
import '@grapecity/spread-sheets-designer';


@Component({
  selector: 'app-spreadjs',
  templateUrl: './spreadjs.component.html',
  styleUrls: ['./spreadjs.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class SpreadjsComponent{
  title = 'en';
  props = {
    styleInfo: "width: 100%; height: 98vh; margin-top: 10px",
    config: null,
    spreadOptions: {sheetCount: 6}
  };



}
