import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HandsontestComponent } from './handsontest.component';

describe('HandsontestComponent', () => {
  let component: HandsontestComponent;
  let fixture: ComponentFixture<HandsontestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HandsontestComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HandsontestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
