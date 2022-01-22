import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowLogsOfTableDialogComponent } from './show-logs-of-table-dialog.component';

describe('ShowLogsOfTableDialogComponent', () => {
  let component: ShowLogsOfTableDialogComponent;
  let fixture: ComponentFixture<ShowLogsOfTableDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ShowLogsOfTableDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowLogsOfTableDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
