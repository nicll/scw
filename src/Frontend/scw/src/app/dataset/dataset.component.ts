import { Component, OnInit } from '@angular/core';
import Handsontable from "handsontable";
import 'handsontable/dist/handsontable.full.css';
import { Column } from '../Models/Column';

@Component({
  selector: 'app-dataset',
  templateUrl: './dataset.component.html',
  styleUrls: ['./dataset.component.scss']
})
export class DatasetComponent implements OnInit {

  data!:Column[];
  constructor() {

  }

  ngOnInit(): void {

  }

}
