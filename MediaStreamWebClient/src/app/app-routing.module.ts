import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MediaListComponent } from './media-list/media-list.component';

const routes: Routes = [
  { path: '', component: MediaListComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
