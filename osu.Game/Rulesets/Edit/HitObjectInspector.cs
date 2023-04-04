// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.TypeExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Screens.Edit;

namespace osu.Game.Rulesets.Edit
{
    internal partial class HitObjectInspector : CompositeDrawable
    {
        private OsuTextFlowContainer inspectorText;

        [Resolved]
        protected EditorBeatmap EditorBeatmap { get; private set; }

        [Resolved]
        private OverlayColourProvider colourProvider { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = inspectorText = new OsuTextFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
            };
        }

        protected override void Update()
        {
            base.Update();
            updateInspectorText();
        }

        private void updateInspectorText()
        {
            inspectorText.Clear();

            switch (EditorBeatmap.SelectedHitObjects.Count)
            {
                case 0:
                    addHeader("No selection");
                    break;

                case 1:
                    var selected = EditorBeatmap.SelectedHitObjects.Single();

                    addHeader("Type");
                    addValue($"{selected.GetType().ReadableName()}");

                    addHeader("Time");
                    addValue($"{selected.StartTime:#,0.##}ms");

                    switch (selected)
                    {
                        case IHasPosition pos:
                            addHeader("Position");
                            addValue($"x:{pos.X:#,0.##} y:{pos.Y:#,0.##}");
                            break;

                        case IHasXPosition x:
                            addHeader("Position");

                            addValue($"x:{x.X:#,0.##} ");
                            break;

                        case IHasYPosition y:
                            addHeader("Position");

                            addValue($"y:{y.Y:#,0.##}");
                            break;
                    }

                    if (selected is IHasDistance distance)
                    {
                        addHeader("Distance");
                        addValue($"{distance.Distance:#,0.##}px");
                    }

                    if (selected is IHasRepeats repeats)
                    {
                        addHeader("Repeats");
                        addValue($"{repeats.RepeatCount:#,0.##}");
                    }

                    if (selected is IHasDuration duration)
                    {
                        addHeader("End Time");
                        addValue($"{duration.EndTime:#,0.##}ms");
                        addHeader("Duration");
                        addValue($"{duration.Duration:#,0.##}ms");
                    }

                    break;

                default:
                    addHeader("Selected Objects");
                    addValue($"{EditorBeatmap.SelectedHitObjects.Count:#,0.##}");

                    addHeader("Start Time");
                    addValue($"{EditorBeatmap.SelectedHitObjects.Min(o => o.StartTime):#,0.##}");

                    addHeader("End Time");
                    addValue($"{EditorBeatmap.SelectedHitObjects.Max(o => o.GetEndTime()):#,0.##}");
                    break;
            }

            void addHeader(string header) => inspectorText.AddParagraph($"{header}: ", s =>
            {
                s.Padding = new MarginPadding { Top = 2 };
                s.Font = s.Font.With(size: 12);
                s.Colour = colourProvider.Content2;
            });

            void addValue(string value) => inspectorText.AddParagraph(value, s =>
            {
                s.Font = s.Font.With(weight: FontWeight.SemiBold);
                s.Colour = colourProvider.Content1;
            });
        }
    }
}
