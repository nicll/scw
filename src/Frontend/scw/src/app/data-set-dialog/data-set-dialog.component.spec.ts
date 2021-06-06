import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataSetDialogComponent } from './data-set-dialog.component';

describe('DataSetDialogComponent', () => {
  let component: DataSetDialogComponent;
  let fixture: ComponentFixture<DataSetDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DataSetDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DataSetDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
