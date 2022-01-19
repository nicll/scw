import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowTablesOfUserDialogComponent } from './show-tables-of-user-dialog.component';

describe('ShowTablesOfUserDialogComponent', () => {
  let component: ShowTablesOfUserDialogComponent;
  let fixture: ComponentFixture<ShowTablesOfUserDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ShowTablesOfUserDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowTablesOfUserDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
