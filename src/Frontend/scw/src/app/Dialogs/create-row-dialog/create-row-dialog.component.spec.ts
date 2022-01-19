import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateRowDialogComponent } from './create-row-dialog.component';

describe('CreateRowDialogComponent', () => {
  let component: CreateRowDialogComponent;
  let fixture: ComponentFixture<CreateRowDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateRowDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateRowDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
