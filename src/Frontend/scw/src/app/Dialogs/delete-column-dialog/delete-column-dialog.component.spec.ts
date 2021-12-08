import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteColumnDialogComponent } from './delete-column-dialog.component';

describe('DeleteColumnDialogComponent', () => {
  let component: DeleteColumnDialogComponent;
  let fixture: ComponentFixture<DeleteColumnDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeleteColumnDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DeleteColumnDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
