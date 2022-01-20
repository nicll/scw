import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { DataSet } from '../Models/DataSet';

@Injectable({
  providedIn: 'root',
})
export class TableService {
  baseUri: string = environment.aspUri+'/api';

  constructor(private http: HttpClient, private snackbar: MatSnackBar) {
  }

  public PostSheet(id: string, table: DataSet): Observable<DataSet> {
    return this.http
      .post<DataSet>(this.baseUri + '/data/sheet/' + id, table, {
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000})
          return throwError(err);
        }),
        map((_) => {
          return table;
        })
      );
  }
  public DeleteSheet(id: string): Observable<DataSet> {
    return this.http
      .delete<any[]>(this.baseUri + '/data/sheet/' + id, {
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000})
          return throwError(err);
        }),
        map((sheet) => {
          return new DataSet(sheet);
        })
      );
  }
  public PostDataSet(id: string, table: Array<any>): Observable<Array<any>> {
    return this.http
      .post<Array<any>>(this.baseUri + '/data/dataset/' + id, table, {
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000})
          return throwError(err);
        }),
        map((_) => {
          return table;
        })
      );
  }
  public DeleteDataSet(id: string): Observable<DataSet> {
    return this.http
      .delete<any[]>(this.baseUri + '/data/dataset/' + id, {
        withCredentials: true,
      })
      .pipe(
        catchError((err) => {
          console.error(err);
          this.snackbar.open("Error in communication with backend", undefined, {duration:5000})
          return throwError(err);
        }),
        map((sheet) => {
          return new DataSet(sheet);
        })
      );
  }
}
