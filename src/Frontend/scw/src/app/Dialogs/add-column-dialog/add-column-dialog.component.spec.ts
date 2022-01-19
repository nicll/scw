import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddColumnDialogComponent } from './add-column-dialog.component';

describe('AddColumnDialogComponent', () => {
  let component: AddColumnDialogComponent;
  let fixture: ComponentFixture<AddColumnDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddColumnDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddColumnDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
