import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-handsontest',
  templateUrl: './handsontest.component.html',
  styleUrls: ['./handsontest.component.scss']
})
export class HandsontestComponent implements OnInit {

  constructor() { }
  dataset: any[]=new Array;
  ngOnInit(): void {
    this.dataset=[['','Row1','Row2','row3'],["testdata","row2","row3"],[1,2,3],[3,2,1]];
  }

}
