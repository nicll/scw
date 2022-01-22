import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowLogsOfUserDialogComponent } from './show-logs-of-user-dialog.component';

describe('ShowLogsOfUserDialogComponent', () => {
  let component: ShowLogsOfUserDialogComponent;
  let fixture: ComponentFixture<ShowLogsOfUserDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ShowLogsOfUserDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowLogsOfUserDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
