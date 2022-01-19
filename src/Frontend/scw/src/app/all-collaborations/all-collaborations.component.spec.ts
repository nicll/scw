import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AllCollaborationsComponent } from './all-collaborations.component';

describe('AllCollaborationsComponent', () => {
  let component: AllCollaborationsComponent;
  let fixture: ComponentFixture<AllCollaborationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AllCollaborationsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AllCollaborationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
